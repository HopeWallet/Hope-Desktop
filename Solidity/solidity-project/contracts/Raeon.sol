pragma solidity ^0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC20/StandardToken.sol";

contract Raeon is StandardToken {

  string public constant name = "Raeon";
  string public constant symbol = "RAEON";
  uint8 public constant decimals = 13;

  uint256 private constant initial_supply = 1333337;

  function Raeon() public {
    totalSupply_ = initial_supply * (10 ** uint256(decimals));
    balances[msg.sender] = totalSupply_;
    emit Transfer(address(0), msg.sender, totalSupply_);
  }

}
