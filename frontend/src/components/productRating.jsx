import React, { Component } from "react";
import { connect } from "react-redux";

class ProductRating extends Component {
  state = {};
  render() {
    return (
      <div className="rating-wrap product-card-item">
        <ul className="rating-stars">
          {
            //TODO: need to change width dynmically (0% - 100%) to fit actual review value
          }
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

export default connect()(ProductRating);
