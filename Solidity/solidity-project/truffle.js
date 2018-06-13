module.exports = {
  networks: {
    development: {
      host: "localhost",
      port: 8545,
      network_id: "*" // Match any network id
    },
    rinkeby: {
      host: "localhost", // Connect to geth on the specified
      port: 8545,
      from: "0xb332Feee826BF44a431Ea3d65819e31578f30446", // default address to use for any transaction Truffle makes during migrations
      network_id: 4,
      gas: 6712390 // Gas limit used for deploys
    }
  }
};
