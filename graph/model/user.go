package model

import "time"

type User struct {
	ID          string     `json:"id"`
	CreatedAt   time.Time  `json:"createdAt"`
	Email       string     `json:"email"`
	Role        string     `json:"role"`
	Status      string     `json:"status"`
	Step        string     `json:"step"`
	FirstName   string     `json:"firstName"`
	LastName    string     `json:"lastName"`
	Situation   string     `json:"situation"`
	Description string     `json:"description"`
	Birthdate   *time.Time `json:"birthdate"`
	Address     *string    `json:"address"`
	Discord     *string    `json:"discord"`
	Linkedin    *string    `json:"linkedin"`
}
