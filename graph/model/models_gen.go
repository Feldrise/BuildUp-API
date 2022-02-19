// Code generated by github.com/99designs/gqlgen, DO NOT EDIT.

package model

import (
	"time"
)

type Filter struct {
	Key   string `json:"key"`
	Value string `json:"value"`
}

type Login struct {
	Email    string `json:"email"`
	Password string `json:"password"`
}

type NewBuilder struct {
	Situation   string      `json:"situation"`
	Description string      `json:"description"`
	Project     *NewProject `json:"project"`
}

type NewCoach struct {
	Situation   string `json:"situation"`
	Description string `json:"description"`
}

type NewProject struct {
	Name                  string    `json:"name"`
	Description           string    `json:"description"`
	Team                  string    `json:"team"`
	Categorie             string    `json:"categorie"`
	Keywords              *string   `json:"keywords"`
	LaunchDate            time.Time `json:"launchDate"`
	IsLucrative           bool      `json:"isLucrative"`
	IsOfficialyRegistered bool      `json:"isOfficialyRegistered"`
}

type NewUser struct {
	Email     string      `json:"email"`
	Password  string      `json:"password"`
	FirstName string      `json:"firstName"`
	LastName  string      `json:"lastName"`
	Builder   *NewBuilder `json:"builder"`
	Coach     *NewCoach   `json:"coach"`
}

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
