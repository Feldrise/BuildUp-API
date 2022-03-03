package config

import (
	"log"
	"os"

	"gopkg.in/yaml.v2"
)

var Cfg Config

type Config struct {
	Settings struct {
		AuthSecret string `yaml:"authSecret"`
	} `yaml:"settings"`
	Database struct {
		Name             string `yaml:"name"`
		ConnectionString string `yaml:"connectionString"`
		Collections      struct {
			Users        string `yaml:"users"`
			Builders     string `yaml:"builders"`
			Coachs       string `yaml:"coachs"`
			Projects     string `yaml:"projects"`
			BuildOns     string `yaml:"buildOns"`
			BuildOnSteps string `yaml:"buildOnSteps"`
		} `yaml:"collections"`
	} `yaml:"database"`
}

// Initialize necessary configuration from a yml file given by
// the config path
func Init(configPath string) {
	file, err := os.Open(configPath)

	if err != nil {
		log.Fatal(err)
	}

	defer file.Close()

	// Since the file is in YAML format we need to decode it
	decoder := yaml.NewDecoder(file)
	err = decoder.Decode(&Cfg)

	if err != nil {
		log.Fatal(err)
	}
}
