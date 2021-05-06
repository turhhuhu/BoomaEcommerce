import React, { Component } from "react";
import { connect } from "react-redux";
import ProfileSideBar from "./profileSideBar";

class UserProfile extends Component {
  state = {
    username: undefined,
    name: undefined,
    lastName: undefined,
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  static getDerivedStateFromProps(props, state) {
    if (state.username === undefined)
      return {
        username: props.username,
        name: props.name,
        lastName: props.lastName,
      };
    return null;
  }

  render() {
    return (
      <div className="ml-5 row">
        <ProfileSideBar isProfile="true" />
        <main className="card col-md-6">
          <div className="card-body">
            <h4 className="card-title mb-4">Profile</h4>
            <form>
              <div className="form-row">
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

export default connect()(UserProfile);
