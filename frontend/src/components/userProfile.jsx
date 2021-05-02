import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserInfo } from "../actions/userActions";

class UserProfile extends Component {
  handleClick = () => {
    this.props.dispatch(fetchUserInfo());
  };

  render() {
    return (
      <div class="card">
        <div class="card-body">
          <h4 class="card-title mb-4">Profile</h4>
          <form>
            <div class="form-group">
              <img
                src="bootstrap-ecommerce-html/images/avatars/avatar1.jpg"
                class="img-sm rounded-circle border"
              ></img>
            </div>
            <div class="form-row">
              <div class="col form-group">
                <label>Name</label>
                <input type="text" class="form-control" value="Mike"></input>
              </div>
              <div class="col form-group">
                <label>Email</label>
                <input
                  type="email"
                  class="form-control"
                  value="Johnson"
                ></input>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group col-md-6">
                <label>Country</label>
                <select id="inputState" class="form-control">
                  <option> Choose...</option>
                  <option>Uzbekistan</option>
                  <option>Russia</option>
                  <option selected="">United States</option>
                  <option>India</option>
                  <option>Afganistan</option>
                </select>
              </div>
              <div class="form-group col-md-6">
                <label>City</label>
                <input type="text" class="form-control"></input>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group col-md-6">
                <label>Zip</label>
                <input type="text" class="form-control" value="123009"></input>
              </div>
              <div class="form-group col-md-6">
                <label>Phone</label>
                <input
                  type="text"
                  class="form-control"
                  value="+123456789"
                ></input>
              </div>
            </div>

            <button class="btn btn-primary btn-block">Save info</button>
          </form>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return { userInfo: store.user.userInfo };
};

export default connect(mapStateToProps)(UserProfile);
