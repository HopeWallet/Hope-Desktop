pragma solidity 0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC20/StandardToken.sol";
import "../node_modules/zeppelin-solidity/contracts/token/ERC20/BurnableToken.sol";
import "../node_modules/zeppelin-solidity/contracts/ownership/rbac/RBAC.sol";

contract DUBI is StandardToken, BurnableToken, RBAC {

  string public constant name = "Decentralized Universal Basic Income";
  string public constant symbol = "DUBI";
  uint8 public constant decimals = 18;
  string constant public ROLE_MINT = "mint";

  event MintLog(address indexed to, uint256 amount);

  function DUBI() public {
    totalSupply_ = 0;
  }

  // used by contracts to mint DUBI tokens
  function mint(address _to, uint256 _amount) external onlyRole(ROLE_MINT) returns (bool) {
    require(_to != address(0));
    require(_amount > 0);

    // update state
    totalSupply_ = totalSupply_.add(_amount);
    balances[_to] = balances[_to].add(_amount);

    // logs
    emit MintLog(_to, _amount);
    emit Transfer(0x0, _to, _amount);
    
    return true;
  }
}