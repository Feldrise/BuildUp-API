package model

type BuildOn struct {
	ID          string `json:"id"`
	Name        string `json:"name"`
	Description string `json:"description"`
	Index       int    `json:"index"`
	AnnexeURL   string `json:"annexeUrl"`
	Rewards     string `json:"rewards"`
}
