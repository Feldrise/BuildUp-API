package users

type UserEmailAlreadyExistsError struct{}
type UserAccessDeniedError struct{}
type UserNotFoundError struct{}
type UserPasswordIncorrectError struct{}
type UserMustHaveRoleError struct{}

func (m *UserEmailAlreadyExistsError) Error() string {
	return "l'email existe déjà"
}

func (m *UserAccessDeniedError) Error() string {
	return "accès non autorisé"
}

func (m *UserNotFoundError) Error() string {
	return "l'utilisateur n'existe pas"
}

func (m *UserPasswordIncorrectError) Error() string {
	return "le mot de passe ne correspond pas"
}

func (m *UserMustHaveRoleError) Error() string {
	return "vous devez attribuer un builder ou un coach à l'utilisateur"
}
