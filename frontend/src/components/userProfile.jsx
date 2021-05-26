import React, { Component } from "react";
import { connect } from "react-redux";
import ProfileSideBar from "./profileSideBar";
import { fetchUserInfo } from "../actions/userActions";
class UserProfile extends Component {
  state = {
    username: "",
    name: "",
    lastName: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  componentDidMount() {
    if (this.props.isAuthenticated) {
      const userInfoPromise = this.props.dispatch(fetchUserInfo());
      userInfoPromise
        .then((action) =>
          this.setState({
            username: action.payload.response.userName,
            name: action.payload.response.name
              ? action.payload.response.name
              : "",
            lastName: action.payload.response.lastName
              ? action.payload.response.lastName
              : "",
          })
        )
        .catch((error) => console.error(error));
    }
  }

  render() {
    return (
      <div className="ml-5 row">
        <ProfileSideBar isProfile="true" />
        <main className="card col-md-6">
          <div className="card-body">
            <div className="border-bottom" style={{ height: "40px" }}>
              <h4 className="card-title mb-4">Profile</h4>
            </div>
            <form>
              <div className="form-row mt-2">
                <div className="form-group col">
                  <label>Username:</label>
                  <input
                    type="text"
                    className="form-control"
                    value={this.state.username}
                    onChange={this.handleChange}
                    name="username"
                    required
                  ></input>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group col">
                  <label>Name:</label>
                  <input
                    type="text"
                    className="form-control"
                    value={this.state.name}
                    onChange={this.handleChange}
                    name="name"
                    required
                  ></input>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group col">
                  <label>Last Name:</label>
                  <input
                    type="text"
                    className="form-control"
                    value={this.state.lastName}
                    onChange={this.handleChange}
                    name="lastName"
                    required
                  ></input>
                </div>
              </div>

              <button className="btn btn-outline-primary btn-block">
                Save info
              </button>
            </form>
          </div>
        </main>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    userInfo: store.user.userInfo,
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(UserProfile);
