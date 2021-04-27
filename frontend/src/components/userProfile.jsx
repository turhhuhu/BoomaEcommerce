import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserInfo } from "../actions/userActions";

class UserProfile extends Component {
  handleClick = () => {
    this.props.dispatch(fetchUserInfo());
  };

  render() {
    return (
      <div>
        <p>{JSON.stringify(this.props.userInfo)}</p>
        <button onClick={this.handleClick}></button>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return { userInfo: store.user.userInfo };
};

export default connect(mapStateToProps)(UserProfile);
