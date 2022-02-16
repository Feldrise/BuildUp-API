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
    createdAt,
    email,
    firstName,
    lastName,
    coach {
      id,
      description
    }
    builder {
      id,
      description
    }
  }
}

query getBuilders {
  builders {
    email,
    firstName,
    lastName,
    builder {
      description,
      coach {
        firstName
      },
      project {
        name,
        description
      }
    }
  }
}

query getCoachs {
  coachs {
    email,
    firstName,
    lastName,
    coach {
      description,
      builders {
        firstName
      }
    }
  }
}

query getBuilder {
  user(id: "620d3324b758ac25982681d8") {
    email,
    firstName,
    lastName,
    builder {
      description,
      coach {
        firstName
      },
      project {
        name,
        description
      }
    }
  }
}

query getCoach {
  user(id: "620ccedca1e77c53925b35e6") {
    email,
    firstName,
    lastName,
    coach {
      description,
      builders {
        firstName,
        builder {
          project {
            name,
            description
          }
        }
      }
    }
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
    lastName: "ASTICOT",
    builder: {
      situation: "Etudiant.e",
      description: "Je suis une super étudiante qui aime plein de truc !",
      project: {
        name: "Le chemin des fées",
        description: "Ce projet à pour but de répertorier tous les endroits entourée de mystères et de légendes qui peuvent se trouver dans le monde.",
        team: "Moi, mon sac et beaucoup de livre",
        categorie: "Voyage",
        keywords: "fée, lieux, voyage, légende",
        launchDate: "2022-03-01T12:00:00.782Z"
        isLucrative: false,
        isOfficialyRegistered: true
      }
    }
  }) {
    id,
    email,
    firstName, 
    builder {
      description
    }
  }
}


mutation createCoach {
  createUser(input: {
    email: "coach@me.com",
    password: "dE8bdTUE",
    firstName: "Guillaume",
    lastName: "EOCHE",
    coach: {
      situation: "Alternant",
      description: "Fondateur de New Talents, je sers aujourd'hui d'exemple dans l'API de BuildUP"
    }
  }) {
    id,
    email,
    firstName,
    coach {
      description
    }
  }
}

# CONNEXION

# Token : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NDUwNTA1MTUsImlkIjoiNjIwYzIzYzIwNzlmZGI3ZTA4YTEwMzJmIn0.DX-Bc44m99GINZMzqn_nncEaQEl7k1lhwLg3u6fGL9c
mutation loginAdmin {
  login(input: {
    email: "admin@me.com",
    password: "dE8bdTUE"
  })
}

# Token : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NDUwNTA1MzAsImlkIjoiNjIwYzIzYzkwNzlmZGI3ZTA4YTEwMzMwIn0.tcEYDdKHtVGOthyr48Wpp7N1mrn6JCteij7pH7bIS3o
mutation loginBuilder {
  login(input: {
    email: "builder@me.com",
    password: "dE8bdTUE"
  })
}

# Token : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NDUwNTA1NDAsImlkIjoiNjIwYzIzY2MwNzlmZGI3ZTA4YTEwMzMxIn0.SoRl2oRo7QoatPtS_Rida0CW21nJg2Wl8l0J0bONrQI
mutation loginCoach {
  login(input: {
    email: "coach@me.com",
    password: "dE8bdTUE"
  })
}

mutation failedLogin1 {
  login(input: {
    email: "admin@me.com",
    password: "dE8bdUE"
  })
}

mutation failedLogin2 {
  login(input: {
    email: "adm@me.com",
    password: "dE8bdTUE"
  })
}
```