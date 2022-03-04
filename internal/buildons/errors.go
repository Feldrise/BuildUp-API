package buildons

type BuildOnNotFoundError struct{}

func (m *BuildOnNotFoundError) Error() string {
	return "le buildon n'a pas été trouvé"
}
