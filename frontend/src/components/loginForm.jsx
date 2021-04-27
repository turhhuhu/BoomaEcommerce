import React, { Component } from "react";
import Button from "@material-ui/core/Button";
import { loginUser } from "../actions/authActions";
import { connect } from "react-redux";

class LoginForm extends Component {
  state = {
    username: "",
    password: "",
  };

  handleSubmit = (event) => {
    event.preventDefault();
    const creds = {
      username: this.state.username.trim(),
      password: this.state.password.trim(),
    };
    this.props.dispatch(loginUser(creds));
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  render() {
    return (
      <div
        className="row col-2 mx-auto card"
        style={{
          marginTop: "200px",
        }}
      >
        <article className="card-body">
          <h4 className="card-title mb-4">Sign in</h4>
          <form onSubmit={this.handleSubmit}>
            <div className="form-group">
              <label>Username</label>
              <input
                name="username"
                value={this.state.username}
                onChange={this.handleChange}
                className="form-control"
                placeholder="Username"
              ></input>
            </div>
            <div className="form-group">
              <label>Password</label>
              <input
                name="password"
                value={this.state.password}
                onChange={this.handleChange}
                className="form-control"
                placeholder="******"
                type="password"
              ></input>
            </div>
            <div className="form-group">
              {/* <label class="custom-control custom-checkbox">
                <input
                  type="checkbox"
                  class="custom-control-input"
                  checked=""
                ></input>
                <div class="custom-control-label"> Remember </div>
              </label> */}
              <div className="form-group">
                <Button
                  variant="contained"
                  type="submit"
                  color="primary"
                  className="btn-block"
                >
                  {" "}
                  Login{" "}
                </Button>
              </div>
            </div>
          </form>
          <p className="text-center">
            Not registered? <a href="/register">Sign Up</a>
          </p>
          <p className="text-center">
            Or you can: <a href="/home">Continue As Guest</a>
          </p>
        </article>
      </div>
    );
  }
}

export default connect()(LoginForm);
