package users

import (
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"golang.org/x/crypto/bcrypt"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/builders"
	"new-talents.fr/buildup/internal/coachs"
	"new-talents.fr/buildup/internal/database"
)

// Roles
const USERROLE_ADMIN = "ADMIN"
const USERROLE_BUILDER = "BUILDER"
const USERROLE_COACH = "COACH"

// Status
const USERSTATUS_CANDIDATING = "CANDIDATING"

// Step
const USERSTEP_DEFAULT = "PRESELECTED"

// Type

type User struct {
	ID           primitive.ObjectID `bson:"_id"`
	CreatedAt    time.Time          `bson:"createdAt"`
	Email        string             `bson:"email"`
	Role         string             `bson:"role"`
	Status       string             `bson:"status"`
	Step         string             `bson:"step"`
	FirstName    string             `bson:"firstName"`
	LastName     string             `bson:"lastName"`
	Situation    string             `bson:"situation"`
	Description  string             `bson:"description"`
	Birthdate    *time.Time         `bson:"birthdate"`
	Address      *string            `bson:"address"`
	Discord      *string            `bson:"discord"`
	Linkedin     *string            `bson:"linkedin"`
	PasswordHash string             `bson:"password_hash"`
}

func (user *User) ToModel() *model.User {
	return &model.User{
		ID:          user.ID.Hex(),
		CreatedAt:   user.CreatedAt,
		Email:       user.Email,
		Role:        user.Role,
		Status:      user.Status,
		Step:        user.Step,
		FirstName:   user.FirstName,
		LastName:    user.LastName,
		Situation:   user.Situation,
		Description: user.Description,
		Birthdate:   user.Birthdate,
		Address:     user.Address,
		Discord:     user.Discord,
		Linkedin:    user.Linkedin,
	}
}

func (user *User) HasRole(role model.Role) bool {
	if user == nil {
		return false
	}

	if user.Role == USERROLE_ADMIN && role == model.RoleAdmin {
		return true
	}
	if user.Role == USERROLE_BUILDER && role == model.RoleBuilder {
		return true
	}
	if user.Role == USERROLE_COACH && role == model.RoleCoach {
		return true
	}

	return false
}

// Creation operation

func Create(input model.NewUser) (*User, error) {
	hashedPassword, err := HashPassword(input.Password)

	if err != nil {
		return nil, err
	}

	// We need to change the role depending on who we create
	userRole := USERROLE_BUILDER

	if input.Coach != nil {
		userRole = USERROLE_COACH
	}

	userObjectID := primitive.NewObjectID()
	userID := userObjectID.Hex()
	databaseUser := User{
		ID:           userObjectID,
		CreatedAt:    time.Now(),
		Email:        input.Email,
		Role:         userRole,
		Status:       USERSTATUS_CANDIDATING,
		Step:         USERSTEP_DEFAULT,
		FirstName:    input.FirstName,
		LastName:     input.LastName,
		Situation:    input.Situation,
		Description:  input.Description,
		Birthdate:    input.Birthdate,
		Address:      input.Address,
		Discord:      input.Discord,
		Linkedin:     input.Linkedin,
		PasswordHash: hashedPassword,
	}

	_, err = database.CollectionUsers.InsertOne(database.MongoContext, databaseUser)

	if err != nil {
		return nil, err
	}

	// Now we need to register the builder/coach
	if input.Builder != nil {
		input.Builder.UserID = &userID
		_, err = builders.Create(*input.Builder)

		if err != nil {
			return &databaseUser, err
		}
	} else if input.Coach != nil {
		input.Coach.UserID = &userID
		_, err = coachs.Create(*input.Coach)

		if err != nil {
			return &databaseUser, err
		}
	}

	return &databaseUser, nil
}

// Update operations
func Update(changes *User) error {
	filter := bson.D{
		primitive.E{
			Key:   "_id",
			Value: changes.ID,
		},
	}

	_, err := database.CollectionUsers.ReplaceOne(database.MongoContext, filter, changes)

	return err
}

// Get operation in database

func GetAll() ([]User, error) {
	filter := bson.D{{}}

	return GetFiltered(filter)
}

func GetForBuilders(builders []builders.Builder) ([]User, error) {
	// We need to cnostruct an array with all the ids
	buildersIDs := []primitive.ObjectID{}

	for _, builder := range builders {
		buildersIDs = append(buildersIDs, builder.UserID)
	}

	// Now we can construct the final filter
	filter := bson.M{
		"_id": bson.M{
			"$in": buildersIDs,
		},
	}

	return GetFiltered(filter)
}

func GetById(id string) (*User, error) {
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

	users, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(users) == 0 {
		return nil, nil
	}

	return &users[0], nil
}

func GetByEmail(email string) (*User, error) {
	filter := bson.D{
		primitive.E{
			Key:   "email",
			Value: email,
		},
	}

	users, err := GetFiltered(filter)

	if err != nil {
		return nil, err
	}

	if len(users) == 0 {
		return nil, nil
	}

	return &users[0], nil
}

func GetByRole(role string) ([]User, error) {
	filter := bson.D{
		primitive.E{
			Key:   "role",
			Value: role,
		},
	}

	return GetFiltered(filter)
}

func GetFiltered(filter interface{}) ([]User, error) {
	users := []User{}

	cursor, err := database.CollectionUsers.Find(database.MongoContext, filter)

	if err != nil {
		return users, err
	}

	for cursor.Next(database.MongoContext) {
		var user User

		err := cursor.Decode(&user)

		if err != nil {
			return users, err
		}

		users = append(users, user)
	}

	if err := cursor.Err(); err != nil {
		return users, err
	}

	return users, nil
}

// Authentication

func Authenticate(login model.Login) bool {
	user, err := GetByEmail(login.Email)

	if user == nil || err != nil {
		return false
	}

	return CheckPasswordHash(login.Password, user.PasswordHash)
}

// Password stuff

func HashPassword(password string) (string, error) {
	bytes, err := bcrypt.GenerateFromPassword([]byte(password), 14)

	return string(bytes), err
}

func CheckPasswordHash(password, hash string) bool {
	err := bcrypt.CompareHashAndPassword([]byte(hash), []byte(password))

	return err == nil
}
