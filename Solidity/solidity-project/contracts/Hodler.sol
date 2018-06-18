pragma solidity 0.4.21;

import "../node_modules/zeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";
import "../node_modules/zeppelin-solidity/contracts/math/SafeMath.sol";
import "../node_modules/zeppelin-solidity/contracts/ownership/Ownable.sol";
import "./Purpose.sol";
import "./DUBI.sol";

contract Hodler is Ownable {

  using SafeMath for uint256;
  using SafeERC20 for Purpose;
  using SafeERC20 for DUBI;

  Purpose public purpose;
  DUBI public dubi;

  struct Item {
    uint256 id;
    address beneficiary;
    uint256 value;
    uint256 releaseTime;
    bool fulfilled;
  }

  mapping(address => mapping(uint256 => Item)) private items;

  function changeDubiAddress(address _dubi) public onlyOwner {
    require(_dubi != address(0));

    dubi = DUBI(_dubi);
  }

  function changePrpsAddress(address _prps) public onlyOwner {
    require(_prps != address(0));

    purpose = Purpose(_prps);
  }

  function hodl(uint256 _id, uint256 _value, uint256 _months) external {
    require(_id > 0);
    require(_value > 0);
    // only 3 types are allowed
    require(_months == 3 || _months == 6 || _months == 12);

    // user
    address _user = msg.sender;

    // get dubi item
    Item storage item = items[_user][_id];
    // make sure dubi doesnt exist already
    require(item.id != _id);

    // turn months to seconds
    uint256 _seconds = _months.mul(2628000);
    // get release time
    uint256 _releaseTime = now.add(_seconds);
    require(_releaseTime > now);

    // check if user has enough balance
    uint256 balance = purpose.balanceOf(_user);
    require(balance >= _value);

    // calculate percentage to mint for user: 3 months = 1% => _months / 3 = x
    uint256 userPercentage = _months.div(3);
    // get dubi amount: => (_value * userPercentage) / 100
    uint256 userDubiAmount = _value.mul(userPercentage).div(100);

    // update state
    items[_user][_id] = Item(_id, _user, _value, _releaseTime, false);

    // transfer tokens to hodler
    assert(purpose.hodlerTransfer(_user, _value));

    // mint tokens for user
    assert(dubi.mint(_user, userDubiAmount));
  }

  function release(uint256 _id) external {
    require(_id > 0);

    // user
    address _user = msg.sender;

    // get item
    Item storage item = items[_user][_id];

    // check if it exists
    require(item.id == _id);
    // check if its not already fulfilled
    require(!item.fulfilled);
    // check time
    require(now >= item.releaseTime);

    // check if there is enough tokens
    uint256 balance = purpose.balanceOf(this);
    require(balance >= item.value);

    // update state
    item.fulfilled = true;

    // transfer tokens to beneficiary
    purpose.safeTransfer(item.beneficiary, item.value);
  }

  function getItem(address _user, uint256 _id) public view returns (uint256, address, uint256, uint256, bool) {
    Item storage item = items[_user][_id];

    return (
      item.id,
      item.beneficiary,
      item.value,
      item.releaseTime,
      item.fulfilled
    );
  }
}