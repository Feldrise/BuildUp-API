# GraphiQL

Ce fichier contient les requêtes créés pour tester l'API. Vous pouvez les copier/coller lorsque vous lancez l'API.

```graphql
query getUsers {
  users {
    id
    createdAt
    email
    firstName
    lastName
    description
    coach {
      id
    }
    builder {
      id
    }
  }
}

query getUser {
  user {
    email
    firstName
    lastName
    role
  }
}

query getBuilders {
  users(
    filters: [{key: "role", value: "BUILDER"}, {key: "status", value: "CANDIDATING"}]
  ) {
    email
    firstName
    lastName
    step
    description
    builder {
      coach {
        firstName
      }
      project {
        name
        description
      }
    }
  }
}

query getCoachs {
  users(filters: [{key: "role", value: "COACH"}]) {
    email
    firstName
    lastName
    description
    coach {
      builders {
        firstName
      }
    }
  }
}

query getBuilder {
  user(id: "6211d866cb19bf019d02f75e") {
    email
    firstName
    lastName
    description
    builder {
      coach {
        firstName
      }
      project {
        name
        description
      }
    }
  }
}

query getCoach {
  user(id: "6211d8c1cb19bf019d02f761") {
    email
    firstName
    lastName
    description
    coach {
      builders {
        firstName
        builder {
          project {
            name
            description
          }
        }
      }
    }
  }
}

mutation createAdmin {
  createUser(
    input: {email: "admin@me.com", password: "dE8bdTUE", firstName: "Victor", lastName: "DENIS", situation: "Créateur", description: "Je suis le créateur de ce back-end !"}
  ) {
    id
    email
    firstName
  }
}

mutation createBuilder {
  createUser(
    input: {email: "builder@me.com", password: "dE8bdTUE", firstName: "Bleuenn", lastName: "ASTICOT", situation: "Fée", description: "Je suis une super étudiante qui aime plein de truc !", birthdate: "2004-06-05T12:00:00.782Z", address: "9 Allée de la Prairie, 29000 La Forêt", discord: "BleuennAsticot#1234", linkedin: "https://linkedin.me/bleuenn", builder: {project: {name: "Le chemin des fées", description: "Ce projet à pour but de répertorier tous les endroits entourée de mystères et de légendes qui peuvent se trouver dans le monde.", team: "Moi, mon sac et beaucoup de livre", categorie: "Voyage", keywords: "fée, lieux, voyage, légende", launchDate: "2022-03-01T12:00:00.782Z", isLucrative: false, isOfficialyRegistered: true}}}
  ) {
    id
    email
    firstName
    builder {
      project {
        name
        description
      }
    }
  }
}

mutation createCoach {
  createUser(
    input: {email: "coach@me.com", password: "dE8bdTUE", firstName: "Guillaume", lastName: "EOCHE", situation: "Alternant", description: "Fondateur de New Talents, je sers aujourd'hui d'exemple dans l'API de BuildUP", coach: {}}
  ) {
    id
    email
    firstName
  }
}

mutation updateBuilder {
  updateUser(
    id: "6211d866cb19bf019d02f75e"
    changes: {firstName: "Bleuenn", status: "CANDIDATING", step: "PRESELECTED", builder: {project: {name: "Le nouveau chemin des fée"}}}
  ) {
    firstName
    builder {
      project {
        name
        description
      }
    }
  }
}

mutation loginAdmin {
  login(input: {email: "admin@me.com", password: "dE8bdTUE"})
}

mutation loginBuilder {
  login(input: {email: "builder@me.com", password: "dE8bdTUE"})
}

mutation loginCoach {
  login(input: {email: "coach@me.com", password: "dE8bdTUE"})
}

mutation failedLogin1 {
  login(input: {email: "admin@me.com", password: "dE8bdUE"})
}

mutation failedLogin2 {
  login(input: {email: "adm@me.com", password: "dE8bdTUE"})
}

query getBuildOns {
  buildons {
    id
    name
    description
    index
    annexeUrl
    rewards
    steps {
      id
      name
    }
  }
}

mutation createBuildOn {
  createBuildOn(
    input: {name: "BuildOn #1", description: "Ceci est le tout premier BuildOn", index: 1, annexeUrl: "https://url.com", rewards: "Toutes nos félicitations"}
  ) {
    id
    name
    description
  }
}

mutation updateBuildOn {
  updateBuildOn(id: "6220c0b1e5fcfb1eed83674a", changes: {name: "BuildOn /1"}) {
    name
  }
}

mutation createBuildOnStep {
  createBuildOnStep(
    buildOnID: "6220c0b1e5fcfb1eed83674a"
    input: {name: "L'étape 2", description: "Une super seconde étape !", index: 2, proofType: "COMMENT", proofDescription: "Vous devez founrnir un commenaire sur oh combien vous aimez Victor"}
  ) {
    name
    description
  }
}

mutation updateBuildOnStep {
  updateBuildOnStep(id: "6220c8e7d45cf394c04a3c82", changes: {name: "L'étape n°1"}) {
    name
  }
}
```