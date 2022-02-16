package projects

type ProjectNotFoundError struct{}

func (m *ProjectNotFoundError) Error() string {
	return "le projet n'a pas été trouvé"
}
