package users

type UserEmailAlreadyExistsError struct{}
type UserNotFoundError struct{}

func (m *UserEmailAlreadyExistsError) Error() string {
	return "l'email existe déjà"
}

func (m *UserNotFoundError) Error() string {
	return "l'utilisateur n'existe pas"
}
