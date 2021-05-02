import React, { Component } from "react";
import Button from "@material-ui/core/Button";
import { RegisterUser } from "../actions/authActions";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import { Alert } from "@material-ui/lab";

class SignUpForm extends Component {
  state = {
    name: "",
    lastName: "",
    username: "",
    password: "",
    error: undefined,
    wasButtonPressed: false,
    loading: false,
  };

  startLoading = () => {
    setTimeout(() => {
      this.setState({ loading: false });
    }, 2000);
  };

  handleSubmit = (event) => {
    event.preventDefault();
    this.setState({ wasButtonPressed: true, error: undefined, loading: true });
    this.startLoading();
    if (
      this.state.lastName === "" ||
      this.state.password === "" ||
      this.state.name === "" ||
      this.state.username === ""
    ) {
      this.setState({ error: "Fields are required" });
      return;
    }
    const userInfo = {
      name: this.state.name,
      lastName: this.state.lastName,
      username: this.state.username.trim(),
      password: this.state.password.trim(),
    };
    this.props.dispatch(RegisterUser(userInfo));
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  render() {
    return (
      <div
        className="row col-4 mx-auto card"
        style={{
          marginTop: "200px",
        }}
      >
        <article className="card-body">
          <header className="mb-4">
            <h4 className="card-title">Sign up</h4>
          </header>
          <form onSubmit={this.handleSubmit}>
            <div className="form-row">
              <div className="col form-group">
                <label>First name</label>
                <input
                  name="name"
                  value={this.state.name}
                  onChange={this.handleChange}
                  className="form-control"
                  placeholder=""
                ></input>
              </div>
              <div className="col form-group">
                <label>Last name</label>
                <input
                  name="lastName"
                  value={this.state.lastName}
                  onChange={this.handleChange}
                  className="form-control"
                  placeholder=""
                ></input>
              </div>
            </div>
            <div className="form-group">
              <label>Username</label>
              <input
                name="username"
                value={this.state.username}
                onChange={this.handleChange}
                className="form-control"
                placeholder=""
              ></input>
            </div>
            <div className="form-group">
              <label>Password</label>
              <input
                name="password"
                value={this.state.password}
                onChange={this.handleChange}
                type="password"
                className="form-control"
                placeholder=""
              ></input>
            </div>
            <div className="form-group mt-3">
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
                  Register{" "}
                </Button>
              )}
            </div>
            {(this.props.error || this.state.error) && (
              <Alert severity="error" onClick={() => this.setState(null)}>
                {this.props.error || this.state.error}
              </Alert>
            )}
            {this.props.isAuthenticated && (
              <div>
                <Alert severity="success" onClick={() => this.setState(null)}>
                  You signed up Successfuly!
                  <br /> redirecing to home page...{" "}
                </Alert>
                {this.state.loading ? null : <Redirect to="/home" />}
              </div>
            )}
          </form>
          <p className="text-center">
            Have an account? <a href="/login">Log In</a>
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

export default connect(mapStateToProps)(SignUpForm);
