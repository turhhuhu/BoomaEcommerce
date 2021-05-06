import React, { Component } from "react";
import { connect } from "react-redux";
import UserProfile from "../components/userProfile";
import Header from "../components/header";
import { fetchUserInfo } from "../actions/userActions";
import { Redirect } from "react-router";

class ProfilePage extends Component {
  state = {};
  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserInfo());
    }
  }
  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <UserProfile
              username={this.props.userInfo.userName}
              name={this.props.userInfo.name}
              lastName={this.props.userInfo.lastName}
            />
          </section>
        </div>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    userInfo: store.user.userInfo,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(ProfilePage);
