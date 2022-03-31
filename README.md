# Quake3ArenaLogReader

C# program that reads a txt file and parse it to json with the following format

[
  {
    "game": 1,
    "status": {
      "total_kills": 0,
      "players": [
        {
          "id": 1,
          "nome": "Isgalamido",
          "kills": 0,
          "old_names": []
        }
      ]
    }
  },
  {
    "game": 2,
    "status": {
      "total_kills": 11,
      "players": [
        {
          "id": 1,
          "nome": "Isgalamido",
          "kills": -5,
          "old_names": []
        },
        {
          "id": 2,
          "nome": "Mocinha",
          "kills": 0,
          "old_names": [
            "Dono da Bola"
          ]
        }
      ]
    }
  },
