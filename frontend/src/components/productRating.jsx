import React, { Component } from "react";
import { connect } from "react-redux";

class ProductRating extends Component {
  state = {};
  render() {
    return (
      <div class="rating-wrap product-card-item">
        <ul class="rating-stars">
          {
            //TODO: need to change width dynmically (0% - 100%) to fit actual review value
          }
          <li style={{ width: "80%" }} class="stars-active">
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
          </li>
          <li>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
            <i class="fa fa-star"></i>
          </li>
        </ul>
        {"  "}
        <small class="label-rating text-muted">4/5</small>
      </div>
    );
  }
}

export default connect()(ProductRating);
