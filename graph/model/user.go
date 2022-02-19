package model

import "time"

type User struct {
	ID        string    `json:"id"`
	CreatedAt time.Time `json:"createdAt"`
	Email     string    `json:"email"`
	Role      string    `json:"role"`
	Step      string    `json:"step"`
	FirstName string    `json:"firstName"`
	LastName  string    `json:"lastName"`
}
