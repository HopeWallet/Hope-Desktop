pragma solidity 0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC20/StandardToken.sol";
import "../node_modules/zeppelin-solidity/contracts/token/ERC20/BurnableToken.sol";
import "../node_modules/zeppelin-solidity/contracts/token/ERC20/MintableToken.sol";
import "../node_modules/zeppelin-solidity/contracts/ownership/rbac/RBAC.sol";

contract Purpose is StandardToken, BurnableToken, MintableToken, RBAC {

  string public constant name = "Purpose";
  string public constant symbol = "PRPS";
  uint8 public constant decimals = 18;
  string constant public ROLE_TRANSFER = "transfer";

  address public hodlerContract;

  function Purpose() public {
    totalSupply_ = 1000000 * (10 ** uint256(decimals));
    balances[msg.sender] = totalSupply_;
    emit Transfer(address(0), msg.sender, totalSupply_);
  }

  function changeHodlerContract(address _hodler) external onlyOwner {
    if (hasRole(hodlerContract, ROLE_TRANSFER)) {
      removeRole(hodlerContract, ROLE_TRANSFER);
    }
    
    hodlerContract = _hodler;
    addRole(hodlerContract, ROLE_TRANSFER);
  }

  // used by hodler contract to transfer users tokens to it
  function hodlerTransfer(address _from, uint256 _value) external onlyRole(ROLE_TRANSFER) returns (bool) {
    require(_from != address(0));
    require(_value > 0);

    // hodler
    address _hodler = msg.sender;

    // update state
    balances[_from] = balances[_from].sub(_value);
    balances[_hodler] = balances[_hodler].add(_value);

    // logs
    emit Transfer(_from, _hodler, _value);

    return true;
  }
}