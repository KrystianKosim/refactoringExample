using System;

namespace LegacyApp
{
    public class UserService
    {
        private ClientRepository _clientRepository;

        public UserService()
        {
            _clientRepository = new ClientRepositoryImpl();
        }

        public UserService(ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!validUserData(firstName, lastName, email, dateOfBirth)) return false;

            var client = getClientById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };

            setUserCreditLimit(client, user);

            if (!checkIsUserHasCorrectLimit(user)) return false;

            UserDataAccess.AddUser(user);
            return true;
        }

        private Client getClientById(int clientId)
        {
            var clientRepository = new ClientRepositoryImpl();
            return clientRepository.GetById(clientId);
        }

        private bool checkIsUserHasCorrectLimit(User user)
        {
            return !user.HasCreditLimit || user.CreditLimit >= 500;
        }

        private void setUserCreditLimit(Client client, User user)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else if (client.Type == "ImportantClient")
            {
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    user.CreditLimit = creditLimit;
                }
            }
            else
            {
                user.HasCreditLimit = true;
                using (var userCreditService = new UserCreditService())
                {
                    int creditLimit = userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                    user.CreditLimit = creditLimit;
                }
            }
        }

        private bool validUserData(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && validEmail(email)
                   && validAge(dateOfBirth);
        }

        private bool validAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            return age >= 21;
        }

        private bool validEmail(string email)
        {
            return email.Contains("@") || email.Contains(".");
        }
    }
}