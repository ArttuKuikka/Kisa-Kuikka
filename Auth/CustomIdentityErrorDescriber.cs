using Microsoft.AspNetCore.Identity;

namespace Kipa_plus.Auth
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"Virhe" }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Optimistic concurrency failure, object has been modified." }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "V‰‰r‰ salasana" }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "V‰‰r‰ token" }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "K‰ytt‰j‰ t‰ll‰ nimell‰ tunnuksella on jo olemassa." }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"K‰ytt‰j‰tunnus '{userName}' on virheellinen, se voi sis‰lt‰‰ vain kirjaimia ja numeroita." }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"S‰hkˆposti '{email}' on virheellinen." }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"K‰ytt‰j‰tunnus '{userName}' on jo olemassa" }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"S‰hkˆposti '{email}' on jo k‰ytˆss‰" }; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Roolin nimi '{role}' on virheellinen" }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Rooli '{role}' on jo olemassa" }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "K‰ytt‰j‰ll‰ on jo salasana" }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "K‰ytt‰j‰ on estetty" }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"K‰ytt‰j‰ on jo roolissa '{role}'." }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"K‰ytt‰j‰ ei ole roolissa '{role}'." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Salasanan t‰ytyy olla v‰hint‰‰n {length} merkki‰ pitk‰." }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Salasanan t‰ytyy sis‰lt‰‰ v‰hint‰‰ yksi numero." }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Salasanan t‰ytyy sis‰lt‰‰ v‰hint‰‰ yksi numero ('0'-'9')." }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Salasanan t‰ytyy sis‰lt‰‰ v‰hint‰‰ yksi pieni kirjain ('a'-'z')." }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Salasanan t‰ytyy sis‰lt‰‰ v‰hint‰‰ yksi iso kirjain ('A'-'Z')." }; }
    }
}