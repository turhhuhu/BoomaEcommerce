import React, { Component } from "react";
import { connect } from "react-redux";
import { logoutUser } from "../actions/authActions";
class UserIcon extends Component {
  state = {};

  handleSignOut = () => {
    this.props.dispatch(logoutUser());
  };

  render() {
    return (
      <div className="widget-header icontext mr-3">
        <a href="/login" className="icon icon-sm rounded-circle border">
          <i className="fa fa-user"></i>
        </a>
        <div className="text">
          <span className="text-muted">
            {this.props.isAuthenticated
              ? "Welcome " + this.props.username + "!"
              : "Welcome Guest!"}{" "}
          </span>
          <div>
            {this.props.isAuthenticated ? (
              <a href="/login" onClick={this.handleSignOut}>
                Sign out
              </a>
            ) : null}
            {this.props.isAuthenticated ? null : <a href="/login">Sign in</a>}
            {this.props.isAuthenticated ? null : " | "}
            {this.props.isAuthenticated ? null : (
              <a href="/register">register</a>
            )}
          </div>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    username: store.auth.username,
  };
};

export default connect(mapStateToProps)(UserIcon);
