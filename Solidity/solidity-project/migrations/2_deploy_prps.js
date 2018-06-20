var Token = artifacts.require("../contracts/Purpose.sol");

module.exports = function(deployer) {
  deployer.deploy(Token);
};
