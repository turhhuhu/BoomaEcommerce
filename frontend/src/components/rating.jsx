import React, { Component } from "react";
import { connect } from "react-redux";

class rating extends Component {
  state = {};
  render() {
    return (
      <div className="rating-wrap">
        <ul className="rating-stars">
          <li
            style={{ width: `${this.props.rating * 10}%` }}
            className="stars-active"
          >
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
          </li>
          <li>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
            <i className="fa fa-star"></i>
          </li>
        </ul>
        {"  "}
        <small className="label-rating text-muted">
          {this.props.rating}/10
        </small>
      </div>
    );
  }
}

export default connect()(rating);
