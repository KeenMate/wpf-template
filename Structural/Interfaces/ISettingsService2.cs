/*
    Copyright (C) 2014-2019 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace WpfTemplate.Structural.Settings {
	/// <summary>
	/// Adds/removes settings
	/// </summary>
	public interface ISettingsService2 : ISettingsService {
		/// <summary>
		/// Reads settings from a file. All current settings are removed
		/// </summary>
		/// <param name="filename">Filename of saved settings</param>
		void Open(string filename);

		/// <summary>
		/// Saves current settings
		/// </summary>
		/// <param name="filename">Filename</param>
		void Save(string filename);
	}
}
