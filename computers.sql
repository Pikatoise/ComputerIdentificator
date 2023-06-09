-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema computers
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema computers
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `computers` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `computers` ;

-- -----------------------------------------------------
-- Table `computers`.`devicetype`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `computers`.`devicetype` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `Name` (`Name` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `computers`.`computer`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `computers`.`computer` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Description` VARCHAR(200) NULL DEFAULT NULL,
  `devicetype` INT NOT NULL,
  `lastUpdate` DATE NOT NULL,
  `windowskey` VARCHAR(50) NULL DEFAULT NULL,
  `Name` VARCHAR(50) NOT NULL,
  `inventoryNum` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `Name` (`Name` ASC) VISIBLE,
  INDEX `computer_devicetype_fk` (`devicetype` ASC) VISIBLE,
  CONSTRAINT `computer_devicetype_fk`
    FOREIGN KEY (`devicetype`)
    REFERENCES `computers`.`devicetype` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 14
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `computers`.`computerspec`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `computers`.`computerspec` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(50) NOT NULL,
  `Value` VARCHAR(100) NULL DEFAULT NULL,
  `IsNetwork` TINYINT(1) NOT NULL DEFAULT '0',
  `computer` INT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `computerspec_computer_fk` (`computer` ASC) VISIBLE,
  CONSTRAINT `computerspec_computer_fk`
    FOREIGN KEY (`computer`)
    REFERENCES `computers`.`computer` (`id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
AUTO_INCREMENT = 185
DEFAULT CHARACTER SET = utf8mb4;

INSERT INTO `computers`.`devicetype`(Name) VALUES (
  `Ноутбук`,
  `ПК`
)

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
