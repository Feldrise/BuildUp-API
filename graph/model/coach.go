package model

import "time"

type Coach struct {
	ID              string    `json:"id"`
	CandidatingDate time.Time `json:"candidatingDate"`
}
