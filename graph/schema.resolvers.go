package graph

// This file will be automatically regenerated based on the schema, any resolver implementations
// will be copied through when generating and any unknown code will be moved to the end.

import (
	"context"
	"fmt"

	"new-talents.fr/buildup/graph/generated"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/builders"
	"new-talents.fr/buildup/internal/users"
	"new-talents.fr/buildup/pkg/jwt"
)

func (r *builderResolver) Coach(ctx context.Context, obj *model.Builder) (*model.User, error) {
	panic(fmt.Errorf("not implemented"))
}

func (r *mutationResolver) CreateUser(ctx context.Context, input model.NewUser) (*model.User, error) {
	if input.Builder == nil && input.Coach == nil {
		return nil, &users.UserMustHaveRoleError{}
	}

	// We first need to check the email isn't taken
	existingUser, err := users.GetByEmail(input.Email)

	if existingUser != nil {
		return nil, &users.UserEmailAlreadyExistsError{}
	}

	if err != nil {
		return nil, err
	}

	databaseUser, err := users.Create(input)

	if err != nil {
		return nil, err
	}

	return databaseUser.ToModel(), nil
}

func (r *mutationResolver) Login(ctx context.Context, input model.Login) (string, error) {
	// We first check the password
	isPasswordCorrect := users.Authenticate(input)

	if !isPasswordCorrect {
		return "", &users.UserPasswordIncorrectError{}
	}

	// Then we can generate the token
	user, err := users.GetByEmail(input.Email)

	if err != nil {
		return "", err
	}

	token, err := jwt.GenerateToken(user.ID.Hex())

	if err != nil {
		return "", err
	}

	return token, nil
}

func (r *queryResolver) Users(ctx context.Context) ([]*model.User, error) {
	databaseUsers, err := users.GetAll()

	if err != nil {
		return nil, err
	}

	users := []*model.User{}

	for _, databaseUser := range databaseUsers {
		user := databaseUser.ToModel()

		users = append(users, user)
	}

	return users, nil
}

func (r *queryResolver) User(ctx context.Context, id string) (*model.User, error) {
	databaseUser, err := users.GetById(id)

	if err != nil {
		return nil, err
	}

	if databaseUser == nil {
		return nil, &users.UserNotFoundError{}
	}

	return databaseUser.ToModel(), nil
}

func (r *queryResolver) Builders(ctx context.Context) ([]*model.User, error) {
	databaseBuilders, err := users.GetByRole(users.USERROLE_BUILDER)

	if err != nil {
		return nil, err
	}

	builders := []*model.User{}

	for _, databaseBuilder := range databaseBuilders {
		builder := databaseBuilder.ToModel()

		builders = append(builders, builder)
	}

	return builders, nil
}

func (r *queryResolver) Coachs(ctx context.Context) ([]*model.User, error) {
	panic(fmt.Errorf("not implemented"))
}

func (r *userResolver) Builder(ctx context.Context, obj *model.User) (*model.Builder, error) {
	databaseBuilder, err := builders.GetForUser(obj.ID)

	if err != nil {
		return nil, err
	}

	if databaseBuilder == nil {
		return nil, nil
	}

	return databaseBuilder.ToModel(), nil
}

func (r *userResolver) Coach(ctx context.Context, obj *model.User) (*model.Coach, error) {
	panic(fmt.Errorf("not implemented"))
}

// Builder returns generated.BuilderResolver implementation.
func (r *Resolver) Builder() generated.BuilderResolver { return &builderResolver{r} }

// Mutation returns generated.MutationResolver implementation.
func (r *Resolver) Mutation() generated.MutationResolver { return &mutationResolver{r} }

// Query returns generated.QueryResolver implementation.
func (r *Resolver) Query() generated.QueryResolver { return &queryResolver{r} }

// User returns generated.UserResolver implementation.
func (r *Resolver) User() generated.UserResolver { return &userResolver{r} }

type builderResolver struct{ *Resolver }
type mutationResolver struct{ *Resolver }
type queryResolver struct{ *Resolver }
type userResolver struct{ *Resolver }
