pragma solidity ^0.4.21;

library Utils {
  function isEther(address addr) internal pure returns (bool) {
    return addr == address(0x0);
  }
}