package builders

import (
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

type Builder struct {
	ID              primitive.ObjectID  `bson:"_id"`
	UserID          primitive.ObjectID  `bson:"userID"`
	CoachID         *primitive.ObjectID `bson:"coachID"`
	CandidatingDate time.Time           `bson:"createdDate"`
	Situation       string              `bson:"situation"`
	Description     string              `bson:"description"`
}

func (builder *Builder) ToModel() *model.Builder {
	var coachID *string

	if builder.CoachID != nil {
		coachIDValue := builder.CoachID.Hex()
		coachID = &coachIDValue
	}

	return &model.Builder{
		ID:              builder.ID.Hex(),
		CandidatingDate: builder.CandidatingDate,
		Situation:       builder.Situation,
		Description:     builder.Description,
		CoachID:         coachID,
	}
}

// Creation operation

func Create(userID primitive.ObjectID, input model.NewBuilder) (*Builder, error) {
	databaseBuilder := Builder{
		ID:              primitive.NewObjectID(),
		UserID:          userID,
		CandidatingDate: time.Now(),
		Situation:       input.Situation,
		Description:     input.Description,
	}

	_, err := database.CollectionBuilders.InsertOne(database.MongoContext, databaseBuilder)

	if err != nil {
		return nil, err
	}

	return &databaseBuilder, nil
}

// Getters operation

func GetAll() ([]Builder, error) {
	filter := bson.D{{}}

	return GetFiltered(filter)
}

func GetById(id string) (*Builder, error) {
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

	builders, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(builders) == 0 {
		return nil, nil
	}

	return &builders[0], nil
}

func GetForUser(userID string) (*Builder, error) {
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

	builders, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(builders) == 0 {
		return nil, nil
	}

	return &builders[0], nil
}

func GetForCoach(coachID string) ([]Builder, error) {
	coachObjectID, err := primitive.ObjectIDFromHex(coachID)

	if err != nil {
		return nil, err
	}

	// We first need to get all the builders
	coachFiler := bson.D{
		primitive.E{
			Key:   "coachID",
			Value: coachObjectID,
		},
	}

	return GetFiltered(coachFiler)
}

func GetFiltered(filter interface{}) ([]Builder, error) {
	builders := []Builder{}

	cursor, err := database.CollectionBuilders.Find(database.MongoContext, filter)

	if err != nil {
		return builders, err
	}

	for cursor.Next(database.MongoContext) {
		var builder Builder

		err := cursor.Decode(&builder)

		if err != nil {
			return builders, err
		}

		builders = append(builders, builder)
	}

	if err := cursor.Err(); err != nil {
		return builders, err
	}

	return builders, nil
}
