var Hodler = artifacts.require("../contracts/Hodler.sol");

module.exports = function(deployer) {
  deployer.deploy(Hodler);
};
