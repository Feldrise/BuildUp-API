package graph

// This file will be automatically regenerated based on the schema, any resolver implementations
// will be copied through when generating and any unknown code will be moved to the end.

import (
	"context"
	"fmt"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"new-talents.fr/buildup/graph/generated"
	"new-talents.fr/buildup/graph/model"
	"new-talents.fr/buildup/internal/auth"
	"new-talents.fr/buildup/internal/builders"
	"new-talents.fr/buildup/internal/coachs"
	"new-talents.fr/buildup/internal/helper"
	"new-talents.fr/buildup/internal/projects"
	"new-talents.fr/buildup/internal/users"
	"new-talents.fr/buildup/pkg/jwt"
)

func (r *builderResolver) Project(ctx context.Context, obj *model.Builder) (*model.Project, error) {
	databaseProjet, err := projects.GetForBuilder(obj.ID)

	if err != nil {
		return nil, err
	}

	if databaseProjet == nil {
		return nil, &projects.ProjectNotFoundError{}
	}

	return databaseProjet.ToModel(), nil
}

func (r *builderResolver) Coach(ctx context.Context, obj *model.Builder) (*model.User, error) {
	if obj.CoachID == nil {
		return nil, nil
	}

	databaseCoach, err := coachs.GetById(*obj.CoachID)

	if err != nil {
		return nil, err
	}

	if databaseCoach == nil {
		return nil, nil
	}

	databaseUser, err := users.GetById(databaseCoach.UserID.Hex())

	if err != nil {
		return nil, err
	}

	if databaseUser == nil {
		return nil, &coachs.UserNotFoundError{}
	}

	return databaseUser.ToModel(), nil
}

func (r *coachResolver) Builders(ctx context.Context, obj *model.Coach) ([]*model.User, error) {
	databaseBuilders, err := builders.GetForCoach(obj.ID)

	if err != nil {
		return nil, err
	}

	databaseUsers, err := users.GetForBuilders(databaseBuilders)

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

func (r *mutationResolver) UpdateUser(ctx context.Context, id string, changes map[string]interface{}) (*model.User, error) {
	// TODO: manage coach and builder update
	user := auth.ForContext(ctx)

	if user.Role != users.USERROLE_ADMIN && user.ID.Hex() != id {
		return nil, &users.UserAccessDeniedError{}
	}

	// User
	databaseUser, err := users.GetById(id)

	if err != nil {
		return nil, err
	}

	if databaseUser == nil {
		return nil, &users.UserNotFoundError{}
	}

	helper.ApplyChanges(changes, databaseUser)

	err = users.Update(databaseUser)

	if err != nil {
		return nil, err
	}

	// Builder
	if changes["builder"] != nil {
		builder := changes["builder"].(map[string]interface{})

		databaseBuilder, err := builders.GetForUser(id)

		if err != nil {
			return nil, err
		}

		if databaseBuilder == nil {
			return nil, &builders.BuilderNotFoundError{}
		}

		if builder["project"] != nil {
			project := builder["project"].(map[string]interface{})

			databaseProject, err := projects.GetForBuilder(databaseBuilder.ID.Hex())

			if err != nil {
				return nil, err
			}

			if databaseProject == nil {
				return nil, &projects.ProjectNotFoundError{}
			}

			helper.ApplyChanges(project, databaseProject)

			err = projects.Update(databaseProject)

			if err != nil {
				return nil, err
			}
		}
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

func (r *queryResolver) Users(ctx context.Context, filters []*model.Filter) ([]*model.User, error) {
	// We need to construct the filter first
	databaseFilter := bson.D{{}}

	for _, filter := range filters {
		databaseFilter = append(databaseFilter, primitive.E{
			Key:   filter.Key,
			Value: filter.Value,
		})
	}

	databaseUsers, err := users.GetFiltered(databaseFilter)

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

func (r *queryResolver) User(ctx context.Context, id *string) (*model.User, error) {
	// If we don't specify any ID, we send the connected user
	if id == nil {
		user := auth.ForContext(ctx)

		return user.ToModel(), nil
	}

	databaseUser, err := users.GetById(*id)

	if err != nil {
		return nil, err
	}

	if databaseUser == nil {
		return nil, &users.UserNotFoundError{}
	}

	return databaseUser.ToModel(), nil
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
	databaseCoach, err := coachs.GetForUser(obj.ID)

	if err != nil {
		return nil, err
	}

	if databaseCoach == nil {
		return nil, nil
	}

	return databaseCoach.ToModel(), nil
}

// Builder returns generated.BuilderResolver implementation.
func (r *Resolver) Builder() generated.BuilderResolver { return &builderResolver{r} }

// Coach returns generated.CoachResolver implementation.
func (r *Resolver) Coach() generated.CoachResolver { return &coachResolver{r} }

// Mutation returns generated.MutationResolver implementation.
func (r *Resolver) Mutation() generated.MutationResolver { return &mutationResolver{r} }

// Query returns generated.QueryResolver implementation.
func (r *Resolver) Query() generated.QueryResolver { return &queryResolver{r} }

// User returns generated.UserResolver implementation.
func (r *Resolver) User() generated.UserResolver { return &userResolver{r} }

type builderResolver struct{ *Resolver }
type coachResolver struct{ *Resolver }
type mutationResolver struct{ *Resolver }
type queryResolver struct{ *Resolver }
type userResolver struct{ *Resolver }

// !!! WARNING !!!
// The code below was going to be deleted when updating resolvers. It has been copied here so you have
// one last chance to move it out of harms way if you want. There are two reasons this happens:
//  - When renaming or deleting a resolver the old code will be put in here. You can safely delete
//    it when you're done.
//  - You have helper methods in this file. Move them out to keep these resolver files clean.
func (r *userResolver) Description(ctx context.Context, obj *model.User) (string, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Situation(ctx context.Context, obj *model.User) (string, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Birthdate(ctx context.Context, obj *model.User) (*time.Time, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Address(ctx context.Context, obj *model.User) (*string, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Discord(ctx context.Context, obj *model.User) (*string, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Linkedin(ctx context.Context, obj *model.User) (*string, error) {
	panic(fmt.Errorf("not implemented"))
}
func (r *userResolver) Step(ctx context.Context, obj *model.User) (string, error) {
	panic(fmt.Errorf("not implemented"))
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
	databaseCoachs, err := users.GetByRole(users.USERROLE_COACH)

	if err != nil {
		return nil, err
	}

	coachs := []*model.User{}

	for _, databaseCoach := range databaseCoachs {
		coach := databaseCoach.ToModel()

		coachs = append(coachs, coach)
	}

	return coachs, nil
}
