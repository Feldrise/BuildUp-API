package model

import "time"

type Builder struct {
	ID              string    `json:"id"`
	CandidatingDate time.Time `json:"candidatingDate"`
	CoachID         *string   `json:"coach"`
}
