package auth

import (
	"context"
	"net/http"

	"new-talents.fr/buildup/internal/users"
	"new-talents.fr/buildup/pkg/jwt"
)

var userCtxKey = contextKey{"user"}

type contextKey struct {
	name string
}

func Middleware() func(http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			header := r.Header.Get("Authorization")

			if header == "" {
				next.ServeHTTP(w, r)
				return
			}

			tokenString := header
			userId, err := jwt.ParseToken(tokenString)

			if err != nil {
				http.Error(w, "Invalid token", http.StatusForbidden)
				return
			}

			user, err := users.GetById(userId)

			if err != nil {
				next.ServeHTTP(w, r)
				return
			}

			ctx := context.WithValue(r.Context(), userCtxKey, user)

			r = r.WithContext(ctx)
			next.ServeHTTP(w, r)
		})
	}
}

func ForContext(ctx context.Context) *users.User {
	raw, _ := ctx.Value(userCtxKey).(*users.User)

	return raw
}
