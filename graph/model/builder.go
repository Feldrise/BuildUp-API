package model

import "time"

type Builder struct {
	CandidatingDate time.Time `json:"candidatingDate"`
	Situation       string    `json:"situation"`
	Description     string    `json:"description"`
	CoachID         *string   `json:"coach"`
}
