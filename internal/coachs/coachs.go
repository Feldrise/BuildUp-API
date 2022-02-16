package coachs

import (
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

type Coach struct {
	ID              primitive.ObjectID `bson:"_id"`
	UserID          primitive.ObjectID `bson:"userID"`
	CandidatingDate time.Time          `bson:"candidatingDate"`
	Situation       string             `bson:"situation"`
	Description     string             `bson:"description"`
}

func (coach *Coach) ToModel() *model.Coach {
	return &model.Coach{
		ID:              coach.ID.Hex(),
		CandidatingDate: coach.CandidatingDate,
		Situation:       coach.Situation,
		Description:     coach.Description,
	}
}

// Creation operation

func Create(userID primitive.ObjectID, input model.NewCoach) (*Coach, error) {
	databaseCoach := Coach{
		ID:              primitive.NewObjectID(),
		UserID:          userID,
		CandidatingDate: time.Now(),
		Situation:       input.Situation,
		Description:     input.Description,
	}

	_, err := database.CollectionCoachs.InsertOne(database.MongoContext, databaseCoach)

	if err != nil {
		return nil, err
	}

	return &databaseCoach, nil
}

// Getters operation

func GetAll() ([]Coach, error) {
	filter := bson.D{{}}

	return GetFiltered(filter)
}

func GetById(id string) (*Coach, error) {
	objectId, err := primitive.ObjectIDFromHex(id)

	if err != nil {
		return nil, err
	}

	filter := bson.D{
		primitive.E{
			Key:   "_id",
			Value: objectId,
		},
	}

	coachs, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(coachs) == 0 {
		return nil, nil
	}

	return &coachs[0], nil
}

func GetForUser(userID string) (*Coach, error) {
	userObjectId, err := primitive.ObjectIDFromHex(userID)

	if err != nil {
		return nil, err
	}

	filter := bson.D{
		primitive.E{
			Key:   "userID",
			Value: userObjectId,
		},
	}

	coachs, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(coachs) == 0 {
		return nil, nil
	}

	return &coachs[0], nil
}

func GetFiltered(filter interface{}) ([]Coach, error) {
	coachs := []Coach{}

	cursor, err := database.CollectionCoachs.Find(database.MongoContext, filter)

	if err != nil {
		return coachs, err
	}

	for cursor.Next(database.MongoContext) {
		var coach Coach

		err := cursor.Decode(&coach)

		if err != nil {
			return coachs, err
		}

		coachs = append(coachs, coach)
	}

	if err := cursor.Err(); err != nil {
		return coachs, err
	}

	return coachs, nil
}
