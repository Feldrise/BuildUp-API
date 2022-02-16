package coachs

type UserNotFoundError struct{}

func (m *UserNotFoundError) Error() string {
	return "aucun utilisateur trouvé pour ce coach"
}
