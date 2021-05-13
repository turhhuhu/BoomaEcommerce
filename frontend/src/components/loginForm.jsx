import React, { Component } from "react";
import Button from "@material-ui/core/Button";
import { loginUser } from "../actions/authActions";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import { Alert } from "@material-ui/lab";

class LoginForm extends Component {
  state = {
    username: "",
    password: "",
    loading: false,
  };

  startLoading = () => {
    setTimeout(() => {
      this.setState({ loading: false });
    }, 2000);
  };

  handleSubmit = (event) => {
    event.preventDefault();
    this.setState({ loading: true });
    this.startLoading();
    if (this.state.username === "" || this.state.password === "") {
      this.setState({ error: "Fields are required" });
      return;
    }
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
    if (!this.state.loading && this.props.isAuthenticated) {
      return <Redirect to="/home" />;
    }
    return (
      <div
        className="row col-2 mx-auto card"
        style={{
          marginTop: "75px",
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
                required
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
                required
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
                {this.props.isFetching || this.props.isAuthenticated ? (
                  <div className="d-flex justify-content-center">
                    <div className="spinner-border text-primary" role="status">
                      <span className="sr-only">Loading...</span>
                    </div>
                  </div>
                ) : (
                  <Button
                    variant="contained"
                    type="submit"
                    color="primary"
                    className="btn-block"
                  >
                    {" "}
                    Login{" "}
                  </Button>
                )}
              </div>
            </div>
            {(this.props.error || this.state.error) && (
              <Alert severity="error" onClick={() => this.setState(null)}>
                {this.props.error || this.state.error}
              </Alert>
            )}
            {this.props.isAuthenticated && (
              <div>
                <Alert severity="success" onClick={() => this.setState(null)}>
                  Successfuly Logged in!
                  <br /> redirecting to home page...{" "}
                </Alert>
              </div>
            )}
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

const mapStateToProps = (store) => {
  return {
    isFetching: store.auth.isFetching,
    isAuthenticated: store.auth.isAuthenticated,
    error: store.auth.error,
  };
};

export default connect(mapStateToProps)(LoginForm);
