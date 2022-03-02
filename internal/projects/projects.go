package projects

import (
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

type Project struct {
	ID                    primitive.ObjectID `bson:"_id"`
	BuilderID             primitive.ObjectID `bson:"builderID"`
	Name                  string             `bson:"name"`
	Description           string             `bson:"description"`
	Team                  string             `bson:"team"`
	Categorie             string             `bson:"categorie"`
	Keywords              string             `bson:"keywords"`
	LaunchDate            time.Time          `bson:"launchDate"`
	IsLucrative           bool               `bson:"isLucrative"`
	IsOfficialyRegistered bool               `bson:"isOfficialyRegistered"`
}

func (project *Project) ToModel() *model.Project {
	return &model.Project{
		ID:                    project.ID.Hex(),
		Name:                  project.Name,
		Description:           project.Description,
		Team:                  project.Team,
		Categorie:             project.Categorie,
		Keywords:              project.Keywords,
		LaunchDate:            project.LaunchDate,
		IsLucrative:           project.IsLucrative,
		IsOfficialyRegistered: project.IsOfficialyRegistered,
	}
}

// Creation operation

func Create(builderID primitive.ObjectID, input model.NewProject) (*Project, error) {
	keywords := ""
	if input.Keywords != nil {
		keywords = *input.Keywords
	}

	databaseProject := Project{
		ID:                    primitive.NewObjectID(),
		BuilderID:             builderID,
		Name:                  input.Name,
		Description:           input.Description,
		Team:                  input.Team,
		Categorie:             input.Categorie,
		Keywords:              keywords,
		LaunchDate:            input.LaunchDate,
		IsLucrative:           input.IsLucrative,
		IsOfficialyRegistered: input.IsOfficialyRegistered,
	}

	_, err := database.CollectionProjects.InsertOne(database.MongoContext, databaseProject)

	if err != nil {
		return nil, err
	}

	return &databaseProject, nil
}

// Update operation

// Update operations
func Update(changes *Project) error {
	filter := bson.D{
		primitive.E{
			Key:   "_id",
			Value: changes.ID,
		},
	}

	_, err := database.CollectionProjects.ReplaceOne(database.MongoContext, filter, changes)

	return err
}

// Getters operation

func GetAll() ([]Project, error) {
	filter := bson.D{{}}

	return GetFiltered(filter)
}

func GetById(id string) (*Project, error) {
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

	projects, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(projects) == 0 {
		return nil, nil
	}

	return &projects[0], nil
}

func GetForBuilder(builderID string) (*Project, error) {
	builderObjectId, err := primitive.ObjectIDFromHex(builderID)

	if err != nil {
		return nil, err
	}

	filter := bson.D{
		primitive.E{
			Key:   "builderID",
			Value: builderObjectId,
		},
	}

	projects, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(projects) == 0 {
		return nil, nil
	}

	return &projects[0], nil
}

func GetFiltered(filter interface{}) ([]Project, error) {
	projects := []Project{}

	cursor, err := database.CollectionProjects.Find(database.MongoContext, filter)

	if err != nil {
		return projects, err
	}

	for cursor.Next(database.MongoContext) {
		var project Project

		err := cursor.Decode(&project)

		if err != nil {
			return projects, err
		}

		projects = append(projects, project)
	}

	if err := cursor.Err(); err != nil {
		return projects, err
	}

	return projects, nil
}
