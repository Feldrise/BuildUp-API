package buildonsteps

type BuildOnStepNotFoundError struct{}

func (m *BuildOnStepNotFoundError) Error() string {
	return "l'étape n'a pas été trouvé"
}
