pragma solidity ^0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC20/MintableToken.sol";

contract Gold is MintableToken {

  string public constant name = "Gold";
  string public constant symbol = "GOLD";
  uint8 public constant decimals = 0;

  function Gold() public {
    totalSupply_ = 10000;
    balances[owner] = totalSupply_;
    emit Transfer(address(0), owner, totalSupply_);
  }

}
