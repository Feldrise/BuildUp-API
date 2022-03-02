package builders

type BuilderNotFoundError struct{}

func (m *BuilderNotFoundError) Error() string {
	return "l'utilisateur n'existe pas"
}
