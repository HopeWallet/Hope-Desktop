pragma solidity ^0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC721/ERC721Token.sol";

contract Item is ERC721Token {

    function Item(string _name, string _symbol) public {
        name_ = _name;
        symbol_ = _symbol;
    }

}