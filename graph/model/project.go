package model

import "time"

type Project struct {
	ID                    string    `json:"id"`
	Name                  string    `json:"name"`
	Description           string    `json:"description"`
	Team                  string    `json:"team"`
	Categorie             string    `json:"categorie"`
	Keywords              string    `json:"keywords"`
	LaunchDate            time.Time `json:"launchDate"`
	IsLucrative           bool      `json:"isLucrative"`
	IsOfficialyRegistered bool      `json:"isOfficialyRegistered"`
}
