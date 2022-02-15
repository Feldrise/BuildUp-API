# GraphiQL

Ce fichier contient les requêtes créés pour tester l'API. Vous pouvez les copier/coller lorsque vous lancez l'API.

```graphql
# Welcome to GraphiQL
#
# GraphiQL is an in-browser tool for writing, validating, and
# testing GraphQL queries.
#
# Type queries into this side of the screen, and you will see intelligent
# typeaheads aware of the current GraphQL type schema and live syntax and
# validation errors highlighted within the text.
#
# GraphQL queries typically start with a "{" character. Lines that start
# with a # are ignored.
#
# An example GraphQL query might look like:
#
#     {
#       field(arg: "value") {
#         subField
#       }
#     }
#
# Keyboard shortcuts:
#
#  Prettify Query:  Shift-Ctrl-P (or press the prettify button above)
#
#     Merge Query:  Shift-Ctrl-M (or press the merge button above)
#
#       Run Query:  Ctrl-Enter (or press the play button above)
#
#   Auto Complete:  Ctrl-Space (or just start typing)
#

##################
## UTILISATEURS ##
##################

# ----- #
# QUERY #
# ----- #

query getUsers {
  users {
    id,
    email,
    firstName,
    lastName,
  }
}

query getUser {
  user(id: "620c23c9079fdb7e08a10330") {
    email,
    firstName,
    lastName,
  }
}

# -------- #
# MUTATION #
# -------- #

# INSCRIPTION

mutation createAdmin {
  createUser(input: {
    email: "admin@me.com",
    password: "dE8bdTUE",
    firstName: "Victor",
    lastName: "DENIS"
  }) {
    id,
    email,
    firstName
  }
}


mutation createBuilder {
  createUser(input: {
    email: "builder@me.com",
    password: "dE8bdTUE",
    firstName: "Bleuenne",
    lastName: "ASTICOT"
  }) {
    id,
    email,
    firstName
  }
}


mutation createCoach {
  createUser(input: {
    email: "coach@me.com",
    password: "dE8bdTUE",
    firstName: "Guillaume",
    lastName: "EOCHE"
  }) {
    id,
    email,
    firstName
  }
}
```