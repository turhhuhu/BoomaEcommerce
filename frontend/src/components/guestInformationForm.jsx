import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import { TextField } from "@material-ui/core";
import { submitGuestInformation } from "../actions/userActions";
import { formatDate } from "../utils/utilFunctions";
import { withRouter } from "react-router";

class GuestInformationForm extends Component {
  state = {
    name: this.props.guestInformation.name
      ? this.props.guestInformation.name
      : "",
    lastName: this.props.guestInformation.lastName
      ? this.props.guestInformation.lastName
      : "",
    dateOfBirth: this.props.guestInformation.dateOfBirth
      ? this.props.guestInformation.dateOfBirth
      : Date.now(),
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleSubmit = () => {
    this.props.dispatch(submitGuestInformation(this.state));
    this.props.history.push("/cart/review");
  };

  render() {
    if (this.props.isAuthenticated) {
      return <Redirect to="/home" />;
    }
    return (
      <div
        className="row col-3 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <article className="card-body">
          <header className="mb-4">
            <h4 className="card-title">Fill up the following information</h4>
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
                  required
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
                  required
                ></input>
              </div>
            </div>
            <div className="form-group">
              <div className="mt-3">
                <label>Date of birth:</label>
                <TextField
                  onChange={this.handleChange}
                  defaultValue={formatDate(this.state.dateOfBirth)}
                  name="dateOfBirth"
                  type="date"
                  InputLabelProps={{
                    shrink: true,
                  }}
                />
              </div>
            </div>
            <div>
              <button
                onClick={this.handleSubmit}
                className="btn btn-primary btn-block"
                type="button"
              >
                {" "}
                Continue to review cart{" "}
              </button>
            </div>
          </form>
        </article>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    guestInformation: store.user.guestInformation,
  };
};

export default withRouter(connect(mapStateToProps)(GuestInformationForm));
