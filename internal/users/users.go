package users

import (
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"golang.org/x/crypto/bcrypt"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/database"
)

const USERROLE_ADMIN = "ADMIN"
const USERROLE_BUILDER = "BUILDER"
const USERROLE_COACH = "COACH"

// Type

type User struct {
	ID           primitive.ObjectID `bson:"_id"`
	CreatedAt    time.Time          `bson:"createAt"`
	Email        string             `bson:"email"`
	Role         string             `bson:"role"`
	FirstName    string             `bson:"firstName"`
	LastName     string             `bson:"lastName"`
	PasswordHash string             `bson:"password_hash"`
}

func (user *User) ToModel() *model.User {
	return &model.User{
		ID:        user.ID.Hex(),
		CreatedAt: user.CreatedAt,
		Email:     user.Email,
		Role:      user.Role,
		FirstName: user.FirstName,
		LastName:  user.LastName,
	}
}

// Creation operation

func Create(input model.NewUser) (*User, error) {
	hashedPassword, err := HashPassword(input.Password)

	if err != nil {
		return nil, err
	}

	databaseUser := User{
		ID:           primitive.NewObjectID(),
		CreatedAt:    time.Now(),
		Email:        input.Email,
		Role:         USERROLE_BUILDER, // TODO: will be modified later
		FirstName:    input.FirstName,
		LastName:     input.LastName,
		PasswordHash: hashedPassword,
	}

	_, err = database.CollectionUsers.InsertOne(database.MongoContext, databaseUser)

	if err != nil {
		return nil, err
	}

	return &databaseUser, nil
}

// Get operation in database

func GetAll() ([]User, error) {
	filter := bson.D{{}}

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
