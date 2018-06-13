var Raeon = artifacts.require("../contracts/Raeon.sol");

module.exports = function(deployer) {
  deployer.deploy(Raeon);
};
